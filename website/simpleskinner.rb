#!/usr/bin/env ruby

require 'ftools'
require 'rexml/document'
require 'xemplate'
require 'uri'
include REXML

BASE_DIR = "."
CONTENT_DIR = File.join(BASE_DIR,"content")
SKIN_DIR = File.join(BASE_DIR,"templates")
OUTPUT_DIR = File.join(BASE_DIR,"output")

TEMPLATE = XEMPLATE::load_template( File.join(SKIN_DIR,"skin.html") )

CONTENT_PATH = "/html/body/div[@id='center']/div[@id='content']"
NAV_LINKS_PATH = "/html/body//div[@id='navigation']/a"

$logger = $stdout
def log( message )
    $logger.puts( message )
    $logger.flush
end

def is_markup( filename )
    filename =~ /.(html)$/
end

def is_cvs_data( filename )
    filename =~ /\/CVS(\/|$)/
end

def is_directory( filename )
    FileTest.directory? filename
end

def list_files dir
    Dir[File.join(dir,"*")].reject {|f| is_cvs_data(f)}
end

def skin_assets
    Dir[File.join(SKIN_DIR,"*")].reject {|f| is_cvs_data(f) or is_markup(f)}
end

def output_file( asset_file, root_asset_dir )
    File.join( OUTPUT_DIR, filename_relative_to(asset_file,root_asset_dir) )
end

def filename_relative_to( file, root_dir )
    if file[0,root_dir.length] != root_dir
        raise "#{file} is not within directory #{root_dir}"
    end
    
    root_dir_length = root_dir.length
    root_dir_length = root_dir_length + 1 if root_dir[-1] != '/'
    
    file[root_dir_length, file.length-root_dir_length]
end

def copy_to_output( asset_file, root_asset_dir )
    dest_file = output_file( asset_file, root_asset_dir )
    
    log "#{asset_file} -> #{dest_file}"
    
    File.copy( asset_file, dest_file )
end

def make_output_directory( asset_dir, root_asset_dir )
    new_dir = output_file( asset_dir, root_asset_dir )
    make_directory(new_dir);
end

def make_directory(new_dir)
    if not File.exists? new_dir
        log "making directory #{new_dir}"
        
        Dir.mkdir( new_dir )
    end
end

def skin_content( content_dir, root_content_dir=content_dir )
    list_files(content_dir).each do |content_file|
        if is_directory(content_file)
            make_output_directory( content_file, root_content_dir )
            skin_content( content_file, root_content_dir )
        elsif is_markup(content_file)
            skin_content_file( content_file, root_content_dir )
        else
                copy_to_output( content_file, root_content_dir )
            end
    end
end

def skin_content_file( content_file, root_content_dir )
    output_file = output_file( content_file, root_content_dir )

    log "#{content_file} ~> #{output_file}"
    
    config = {
        "content" => content_file,
        "isindex" => (content_file =~ /content\/index\.html$/) != nil
    }

    skinned_content = TEMPLATE.expand( config )

    add_print_footnotes( skinned_content )

    set_selected_link(skinned_content, content_file)

    write_to_output( skinned_content, output_file )
end

def set_selected_link(skinned_content, content_file)

    skinned_content.each_element "#{NAV_LINKS_PATH}" do |link|
        id = (link.attributes["id"]).to_s
        
        if(content_file.to_s.match(id))
            link.add_attribute("class", "selected")
        end
    end
end

def add_print_footnotes( skinned_content )
	footnotes = {}
	
    footnotes_div = Element.new("div")
    footnotes_div.add_attribute( "class", "LinkFootnotes" )
    footnotes_div.add_element( "p", {"class", "LinkFootnotesHeader"} ).add_text("Links:");
	
    skinned_content.each_element "#{CONTENT_PATH}//a[@href]" do |link|
        href = BASE_URL.merge( link.attributes["href"] ).to_s
        
        if footnotes.has_key? href
        	footnote_index = footnotes[href]
        else
        	footnote_index = footnotes.size + 1
        	footnotes[href] = footnote_index
        	
	        footnote = footnotes_div.add_element("p")
	        footnote.add_text( "#{footnote_index}. " );
	        footnote.add_element( "a", {"href" => href } ).add_text( href )
        end
		
        footnote_ref = Element.new("span")
        footnote_ref.add_attribute( "class", "LinkFootnoteRef" )
        footnote_ref.add_element("sup").add_text(footnote_index.to_s)
        link.parent.insert_after( link, footnote_ref )
    end
	
    if footnotes_div.size > 1
        skinned_content.elements[CONTENT_PATH].add( footnotes_div )
    end
end

def write_to_output( xhtml, output_file )
    File.open( output_file, "w" ) do |output|
        xhtml.write( output, -1, true, true )
    end
end

def build_site
    make_directory OUTPUT_DIR
    skin_content CONTENT_DIR
    skin_assets.each {|f| copy_to_output f, SKIN_DIR}
end

begin
    build_site
    $stdout.puts "done"
    $stdout.flush
rescue REXML::ParseException => ex
    $stderr.puts "parse error: " + ex.message
    $stderr.flush
    
    exit 1
end

